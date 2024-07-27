import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Region } from './region.model';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RegionService {
  private apiUrl = `${environment.apiUrl}/api/regions`;

  constructor(private http: HttpClient) { }

  /**
   * Get all regions.
   */
  getAllRegions(): Observable<Region[]> {
    return this.http.get<Region[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }


  getRegionByDDD(ddd: string): Observable<Region> {
    const url = `${this.apiUrl}/${ddd}`;
    return this.http.get<Region>(url).pipe(
      catchError(this.handleError)
    );
  }


  createRegion(region: Region): Observable<Region> {
    return this.http.post<Region>(this.apiUrl, region).pipe(
      catchError(this.handleError)
    );
  }


  updateRegion(region: Region): Observable<void> {
    const url = `${this.apiUrl}/${region.ddd}`;
    return this.http.put<void>(url, region).pipe(
      catchError(this.handleError)
    );
  }


  deleteRegion(ddd: string): Observable<void> {
    const url = `${this.apiUrl}/${ddd}`;
    return this.http.delete<void>(url).pipe(
      catchError(this.handleError)
    );
  }


  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `An error occurred: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Server returned code: ${error.status}, error message is: ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(errorMessage);
  }
}
