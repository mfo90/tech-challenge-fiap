import { Component, OnInit } from '@angular/core';
import { ContactService } from '../../../services/contact.service';
import { Contact } from '../../../services/contact.model';

@Component({
  selector: 'app-contact-list',
  templateUrl: './contact-list.component.html',
  styleUrls: ['./contact-list.component.css']
})
export class ContactListComponent implements OnInit {
  contacts: Contact[] = [];
  searchDDD: string = '';  // Adicionado campo para DDD

  constructor(private contactService: ContactService) { }

  ngOnInit(): void {
    this.getContacts();
  }

  getContacts(): void {
    this.contactService.getContacts().subscribe((data: Contact[]) => {
      this.contacts = data;
      console.log(this.contacts); // Verificar os dados recebidos no console
    });
  }

  searchContacts(): void {
    if (this.searchDDD.trim() !== '') {
      this.contactService.getContactsByDDD(this.searchDDD).subscribe((data: Contact[]) => {
        this.contacts = data;
      });
    } else {
      this.getContacts();  // Se o campo de pesquisa estiver vazio, retornar todos os contatos
    }
  }

  deleteContact(id: number): void {
    if (confirm('Tem certeza de que deseja deletar este contato?')) {
      this.contactService.deleteContact(id).subscribe(() => {
        this.contacts = this.contacts.filter(contact => contact.id !== id);
      });
    }
  }
  
}
