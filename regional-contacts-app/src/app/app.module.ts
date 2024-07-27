import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { RegionListComponent } from './regions/region-list/region-list.component';
import { RegionAddComponent } from './regions/region-add/region-add.component';
import { RegionEditComponent } from './regions/region-edit/region-edit.component';
import { AppRoutingModule } from './app.routes';
import { RegionService } from '../services/region.service';
import { AuthInterceptor } from './auth.interceptor';
import { ContactListComponent } from './contacts/contact-list/contact-list.component';
import { ContactEditComponent } from './contacts/contact-edit/contact-edit.component';
import { ContactAddComponent } from './contacts/contact-add/contact-add.component';
import { LoginComponent } from './login/login.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegionListComponent,
    RegionAddComponent,
    RegionEditComponent,
    ContactListComponent,
    ContactAddComponent,
    ContactEditComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule,
  ],
  providers: [
    RegionService,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
