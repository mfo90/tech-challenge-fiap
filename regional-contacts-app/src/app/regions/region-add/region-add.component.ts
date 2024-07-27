import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Region } from '../../../services/region.model';
import { RegionService } from '../../../services/region.service';

declare var bootstrap: any;

@Component({
  selector: 'app-region-add',
  templateUrl: './region-add.component.html',
  styleUrls: ['./region-add.component.css']
})
export class RegionAddComponent {
  region: Region = { ddd: '', name: '' };
  regionToConfirm: Region | null = null;
  successMessage: string | null = null;

  constructor(private regionService: RegionService, private router: Router) { }

  confirmAddRegion() {
    this.regionToConfirm = { ...this.region };
    const modalElement = document.getElementById('confirmModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  addRegion() {
    if (this.regionToConfirm) {
      this.regionToConfirm.ddd = this.regionToConfirm.ddd.toString()
      this.regionService.createRegion(this.regionToConfirm).subscribe(() => {
        this.successMessage = 'Region added successfully';
        this.region = { ddd: '', name: '' }; // Reset the form
        this.regionToConfirm = null;
        setTimeout(() => {
          const modalElement = document.getElementById('confirmModal');
          if (modalElement) {
            const modal = bootstrap.Modal.getInstance(modalElement);
            modal.hide();
          }
          this.router.navigate(['/regions']); // Redirecionar para a lista de regiões
        }, 0); // Espera 2 segundos antes de redirecionar para mostrar a mensagem de sucesso
      });
    }
  }

  closeModal() {
    const modalElement = document.getElementById('confirmModal');
    if (modalElement) {
      const modal = bootstrap.Modal.getInstance(modalElement);
      modal.hide();
    }
    this.successMessage = null; // Reset the success message
  }
}
