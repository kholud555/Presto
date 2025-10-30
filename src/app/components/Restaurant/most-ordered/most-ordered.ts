import {
  Component,
  Input,
  OnInit,
  OnChanges,
  SimpleChanges,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-most-ordered',
  templateUrl: './most-ordered.html',
  styleUrls: ['./most-ordered.css'],
  standalone: true,
  imports: [CommonModule, MatCardModule],
})
export class MostOrdered implements OnInit, OnChanges {
  @Input() restaurantID!: string;
  mostOrdered: any[] = [];

  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private baseUrl = 'https://prestoordering.somee.com/api';

  ngOnInit() {
    if (this.restaurantID) {
      this.loadMostOrdered();
    } else {
      this.mostOrdered = [];
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['restaurantID'] && !changes['restaurantID'].isFirstChange()) {
      if (this.restaurantID) {
        this.loadMostOrdered();
      } else {
        this.mostOrdered = [];
      }
    }
  }

  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('authToken');
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  private loadMostOrdered(): void {
    if (!this.restaurantID) {
      this.mostOrdered = [];
      return;
    }
    console.log("Restaurant ID", this.restaurantID);
    const headers = this.getAuthHeaders();

    this.http.get<any>(`${this.baseUrl}/item/${this.restaurantID}/items/most-ordered`, { headers }).subscribe({
  next: (data) => {
    console.log("data:", data);
    this.mostOrdered = Array.isArray(data.$values) ? data.$values : [];
  },
  error: () => {
    this.mostOrdered = [];
  },
});
  }

  public getImageUrl(imageFile?: string): string {
  const url = this.authService.getImageUrl(imageFile);
  return url;
}
}
