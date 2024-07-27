import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Contact } from './contact.model';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  private apiUrl = `${environment.apiUrl}/api/contacts`;

  constructor(private http: HttpClient) { }

  getContacts(): Observable<Contact[]> {
    return this.http.get<Contact[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  getContact(id: number): Observable<Contact> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<Contact>(url).pipe(
      catchError(this.handleError)
    );
  }

  createContact(contact: Contact): Observable<Contact> {
    return this.http.post<Contact>(this.apiUrl, contact).pipe(
      catchError(this.handleError)
    );
  }

  updateContact(contact: Contact): Observable<void> {
    const url = `${this.apiUrl}/${contact.id}`;
    return this.http.put<void>(url, contact).pipe(
      catchError(this.handleError)
    );
  }

  deleteContact(id: number): Observable<void> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.delete<void>(url).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `An error occurred: ${error.error.message}`;
    } else {
      errorMessage = `Server returned code: ${error.status}, error message is: ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(errorMessage);
  }

  getContactsByDDD(ddd: string): Observable<Contact[]> {
    return this.http.get<Contact[]>(`${this.apiUrl}?ddd=${ddd}`);
  }
}
