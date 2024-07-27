import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ContactService } from '../../../services/contact.service';
import { RegionService } from '../../../services/region.service';
import { Contact } from '../../../services/contact.model';
import { Region } from '../../../services/region.model';

declare var bootstrap: any;

@Component({
  selector: 'app-contact-add',
  templateUrl: './contact-add.component.html',
  styleUrls: ['./contact-add.component.css']
})
export class ContactAddComponent implements OnInit {
  contact: Contact = { id: 0, name: '', phone: '', email: '', ddd: '' };
  contactToConfirm: Contact | null = null;
  successMessage: string | null = null;
  errorMessage: string | null = null; // Adicionada variável para mensagens de erro
  regions: Region[] = [];

  constructor(
    private contactService: ContactService,
    private regionService: RegionService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.regionService.getAllRegions().subscribe((data: Region[]) => {
      this.regions = data;
    });
  }

  confirmAddContact() {
    this.contactToConfirm = { ...this.contact };
    const modalElement = document.getElementById('confirmModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  addContact() {
    if (this.contactToConfirm) {
      this.contactService.createContact(this.contactToConfirm).subscribe(
        () => {
          this.successMessage = 'Contato adicionado com sucesso';
          this.errorMessage = null; // Resetar mensagem de erro em caso de sucesso
          this.contact = { id: 0, name: '', phone: '', email: '', ddd: '' }; // Resetar o formulário
          this.contactToConfirm = null;
          setTimeout(() => {
            const modalElement = document.getElementById('confirmModal');
            if (modalElement) {
              const modal = bootstrap.Modal.getInstance(modalElement);
              modal.hide();
            }
            this.router.navigate(['/contacts']); // Redirecionar para a lista de contatos
          }, 0); // Espera 2 segundos antes de redirecionar para mostrar a mensagem de sucesso
        },
        (error) => {
          this.errorMessage = 'Erro ao adicionar contato: ' + (error.error || 'Erro desconhecido'); // Capturar a mensagem de erro
          this.successMessage = null; // Resetar mensagem de sucesso em caso de erro
        }
      );
    }
  }

  closeModal() {
    const modalElement = document.getElementById('confirmModal');
    if (modalElement) {
      const modal = bootstrap.Modal.getInstance(modalElement);
      modal.hide();
    }
    this.successMessage = null; // Resetar a mensagem de sucesso
    this.errorMessage = null; // Resetar a mensagem de erro
  }
}
