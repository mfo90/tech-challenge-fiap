import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ContactService } from '../../../services/contact.service';
import { RegionService } from '../../../services/region.service';
import { Contact } from '../../../services/contact.model';
import { Region } from '../../../services/region.model';

@Component({
  selector: 'app-contact-edit',
  templateUrl: './contact-edit.component.html',
  styleUrls: ['./contact-edit.component.css']
})
export class ContactEditComponent implements OnInit {
  contact: Contact = { id: 0, name: '', phone: '', email: '', ddd: '' };
  regions: Region[] = [];
  errorMessage: string | null = null; // Adicionada variável para mensagem de erro
  successMessage: string | null = null; // Adicionada variável para mensagem de sucesso

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private contactService: ContactService,
    private regionService: RegionService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.contactService.getContact(+id).subscribe((data: Contact) => {
        this.contact = data;
      });
    }
    this.regionService.getAllRegions().subscribe((data: Region[]) => {
      this.regions = data;
    });
  }

  updateContact(): void {
    this.contactService.updateContact(this.contact).subscribe(
      () => {
        this.successMessage = 'Contato atualizado com sucesso';
        this.errorMessage = null; // Resetar mensagem de erro em caso de sucesso
        setTimeout(() => {
          this.router.navigate(['/contacts']);
        }, 2000); // Redirecionar após 2 segundos para mostrar a mensagem de sucesso
      },
      (error) => {
        this.errorMessage = 'Erro ao atualizar contato: ' + (error.error || 'Erro desconhecido');
        this.successMessage = null; // Resetar mensagem de sucesso em caso de erro
      }
    );
  }
}
