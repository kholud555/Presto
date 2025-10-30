import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';
import {
  DeliveryOrderDTO,
  NotificationDTO,
  OrderStatus,
  UpdateOrderStatusRequest,
} from '../../models/DeliveryManDashboardInterface/delivery-order.interface.ts';
import { AuthService } from './../auth';

@Injectable({
  providedIn: 'root',
})
export class DeliveryService {
  private readonly baseUrl = 'https://prestoordering.somee.com/api';
  private hubConnection!: signalR.HubConnection;

  // State management
  private currentOrderSubject = new BehaviorSubject<DeliveryOrderDTO | null>(
    null
  );
  private completedOrdersSubject = new BehaviorSubject<DeliveryOrderDTO[]>([]);
  private notificationsSubject = new BehaviorSubject<NotificationDTO[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  // Public observables
  public currentOrder$ = this.currentOrderSubject.asObservable();
  public completedOrders$ = this.completedOrdersSubject.asObservable();
  public notifications$ = this.notificationsSubject.asObservable();
  public loading$ = this.loadingSubject.asObservable();

  constructor(private http: HttpClient, private authService: AuthService) {
    this.initializeSignalR();
    this.fetchCompletedOrders();
  }

  private initializeSignalR(): void {
    const token = this.authService.getAuthToken();

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://prestoordering.somee.com/notificationHub', {
        accessTokenFactory: () => token || '',
      })
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR Connected ✅');

        this.hubConnection.on(
          'ReceiveNotification',
          (notification: NotificationDTO) => {
            this.addNotification(notification);
            if (notification.message.includes('order assigned')) {
              this.fetchCurrentOrder();
            }
          }
        );
      })
      .catch((err) => console.error('SignalR Connection Error: ', err));
  }

  private unwrapNetResponse(data: any): any {
    if (data && data.$values && Array.isArray(data.$values)) {
      return data.$values.map((item: any) => this.unwrapNetResponse(item));
    } else if (data && typeof data === 'object') {
      const unwrapped: any = {};
      for (const key in data) {
        if (key !== '$id') {
          if (data[key] && data[key].$values) {
            unwrapped[key] = this.unwrapNetResponse(data[key]);
          } else {
            unwrapped[key] = data[key];
          }
        }
      }
      return unwrapped;
    }
    return data;
  }

  // =========================
  // 📌 Helper method لجلب orders حسب الـ status
  // =========================
  private fetchOrdersByStatus(
    statusEnum: number
  ): Observable<DeliveryOrderDTO[]> {
    console.log(`🔍 Fetching orders with status: ${statusEnum}`);

    return this.http
      .get<any>(`${this.baseUrl}/Order/PreparingOrdersForDelivary`, {
        headers: this.authService.getAuthHeaderDeleviry(),
        params: {
          DeliveringOrderStatus: statusEnum.toString(),
        },
      })
      .pipe(
        map((response) => {
          console.log(response)
          const unwrappedData = this.unwrapNetResponse(response);
          console.log(
            `📊 API Response for status ${statusEnum}:`,
            unwrappedData
          );
          return unwrappedData as DeliveryOrderDTO[];
        }),
        catchError((error) => {
          console.error(
            `❌ Error fetching orders with status ${statusEnum}:`,
            error
          );
          return of([]);
        })
      );
  }

  // =========================
  // 📌 Fetch current order
  // =========================
  fetchCurrentOrder(): void {
    this.loadingSubject.next(true);
    console.log('🔄 Fetching current order...');

    const preparingOrders$ = this.fetchOrdersByStatus(2);
    const outForDeliveryOrders$ = this.fetchOrdersByStatus(3);

    forkJoin([preparingOrders$, outForDeliveryOrders$]).subscribe({
      next: ([preparingOrders, outForDeliveryOrders]) => {
        console.log('📋 Preparing orders:', preparingOrders);
        console.log('🚚 Out for delivery orders:', outForDeliveryOrders);

        let currentOrder: DeliveryOrderDTO | null = null;

        if (outForDeliveryOrders && outForDeliveryOrders.length > 0) {
          currentOrder = outForDeliveryOrders[0];
        } else if (preparingOrders && preparingOrders.length > 0) {
          currentOrder = preparingOrders[0];
        }

        console.log('🎯 Current order selected:', currentOrder);

        this.currentOrderSubject.next(currentOrder);
        this.loadingSubject.next(false);

        if (currentOrder) {
          const statusText = this.getStatusText(currentOrder.status || 2);
          this.addNotification({
            userId: this.authService.getUserId() || 'system',
            message: `📋 Active order found: #${currentOrder.orderNumber} - ${statusText}`,
          });
        }
      },
      error: (error) => {
        console.error('❌ Error fetching current order:', error);
        this.loadingSubject.next(false);
        this.addNotification({
          userId: 'system',
          message:
            'Failed to fetch orders. Please check your connection and try again.',
        });
      },
    });
  }

  // =========================
  // 📌 Fetch completed orders
  // =========================
  fetchCompletedOrders(): void {
    console.log('🔄 Fetching completed orders...');

    this.fetchOrdersByStatus(4).subscribe({
      next: (orders: DeliveryOrderDTO[]) => {
        console.log('✅ Completed orders fetched:', orders.length);
        this.completedOrdersSubject.next(orders || []);
      },
      error: (error) => {
        console.error('❌ Error fetching completed orders:', error);
        this.completedOrdersSubject.next([]);
        this.addNotification({
          userId: 'system',
          message: 'Failed to fetch completed orders.',
        });
      },
    });
  }

  // =========================
  // 📌 Update order status
  // =========================
  updateOrderStatus(
    orderID: string,
    status: OrderStatus,
    deliveryManId?: string
  ): Observable<any> {
    const finalDeliveryManId = deliveryManId || this.authService.getUserId();

    if (!finalDeliveryManId) {
      return new Observable((observer) => {
        observer.error(
          new Error(
            'DeliveryManId is required but not found. Please log in again.'
          )
        );
      });
    }

    const request: UpdateOrderStatusRequest = {
      orderID,
      status: Number(status), // تأكد أنه رقم
      deliveryManId: finalDeliveryManId,
    };

    console.log('📤 Final request to backend:', request);

    return this.http.patch(
      `${this.baseUrl}/DeliveryMan/UpdateOrderStatus`,
      request,
      { headers: this.authService.getAuthHeaderDeleviry() }
    );
  }

  // =========================
  // 📌 Progress order
  // =========================
  progressOrder(deliveryManId?: string): void {
    const currentOrder = this.currentOrderSubject.value;
    if (!currentOrder) {
      this.addNotification({
        userId: 'system',
        message: 'No active order found to update.',
      });
      return;
    }

    const currentStatus = Number(currentOrder.status) || 2;
    const nextStatus = this.getNextStatus(currentStatus);

    console.log('⏭️ Progressing order...');
    console.log('   Current status:', currentStatus);
    console.log('   Next status:', nextStatus);

    this.loadingSubject.next(true);

    this.updateOrderStatus(
      currentOrder.orderID,
      nextStatus,
      deliveryManId
    ).subscribe({
      next: (response) => {
        console.log('✅ Order status updated successfully:', response);

        if (nextStatus === 4) {
          this.currentOrderSubject.next(null);
          this.addNotification({
            userId: this.authService.getUserId() || 'system',
            message: `✅ Order #${currentOrder.orderNumber} delivered successfully!`,
          });
          this.fetchCompletedOrders();
        } else {
          const updatedOrder = {
            ...currentOrder,
            status: nextStatus,
          };
          this.currentOrderSubject.next(updatedOrder);

          this.addNotification({
            userId: this.authService.getUserId() || 'system',
            message: `📋 Order #${
              currentOrder.orderNumber
            } is now ${this.getStatusText(nextStatus)}`,
          });
        }
        this.loadingSubject.next(false);
      },
      error: (error) => {
        console.error('❌ Error updating order status:', error);
        this.loadingSubject.next(false);
        this.addNotification({
          userId: 'system',
          message: 'Failed to update order status. Please try again.',
        });
      },
    });
  }

  // =========================
  // 📌 Helpers
  // =========================
  private getNextStatus(currentStatus: OrderStatus): OrderStatus {
    switch (currentStatus) {
      case OrderStatus.Preparing:
        return OrderStatus.Out_for_Delivery;
      case OrderStatus.Out_for_Delivery:
        return OrderStatus.Delivered;
      default:
        return currentStatus;
    }
  }

  getStatusText(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Preparing:
        return 'Preparing';
      case OrderStatus.Out_for_Delivery:
        return 'On Route';
      case OrderStatus.Delivered:
        return 'Delivered';
      default:
        return 'Unknown';
    }
  }

  getNextButtonText(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Preparing:
        return 'Start Delivery';
      case OrderStatus.Out_for_Delivery:
        return 'Mark as Delivered';
      default:
        return 'Complete';
    }
  }

  debugFetchOrdersByStatus(status: number): void {
    console.log(`🔍 Debug: Fetching orders with status: ${status}`);
    this.fetchOrdersByStatus(status).subscribe({
      next: (orders) => {
        console.log(`📊 Orders found for status ${status}:`, orders);
        console.table(orders);
      },
      error: (error) => {
        console.error(`❌ Error fetching orders for status ${status}:`, error);
      },
    });
  }

  // =========================
  // 📌 Notifications
  // =========================
  private addNotification(notification: NotificationDTO): void {
    const current = this.notificationsSubject.value;
    this.notificationsSubject.next([notification, ...current]);
  }

  clearNotifications(): void {
    this.notificationsSubject.next([]);
  }

  sendNotification(notification: NotificationDTO): Observable<any> {
    return this.http.post(`${this.baseUrl}/Notification/Notify`, notification);
  }
}
