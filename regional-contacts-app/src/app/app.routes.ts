import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegionListComponent } from './regions/region-list/region-list.component';
import { RegionAddComponent } from './regions/region-add/region-add.component';
import { RegionEditComponent } from './regions/region-edit/region-edit.component';
import { ContactListComponent } from './contacts/contact-list/contact-list.component';
import { ContactAddComponent } from './contacts/contact-add/contact-add.component';
import { ContactEditComponent } from './contacts/contact-edit/contact-edit.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './auth.guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', component: RegionListComponent, canActivate: [AuthGuard] },
  { path: 'regions', component: RegionListComponent, canActivate: [AuthGuard] },
  { path: 'regions/add', component: RegionAddComponent, canActivate: [AuthGuard] },
  { path: 'regions/edit/:ddd', component: RegionEditComponent, canActivate: [AuthGuard] },
  { path: 'contacts', component: ContactListComponent, canActivate: [AuthGuard] },
  { path: 'contacts/add', component: ContactAddComponent, canActivate: [AuthGuard] },
  { path: 'contacts/edit/:id', component: ContactEditComponent, canActivate: [AuthGuard] },
  // Adicione uma rota para redirecionar para 'regions' se o caminho não corresponder a nenhuma rota acima
  { path: '**', redirectTo: '/regions' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
