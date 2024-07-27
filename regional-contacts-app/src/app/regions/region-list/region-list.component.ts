import { Component, OnInit } from '@angular/core';
import { RegionService } from '../../../services/region.service';
import { Region } from '../../../services/region.model';

@Component({
  selector: 'app-region-list',
  templateUrl: './region-list.component.html',
  styleUrls: ['./region-list.component.css']
})
export class RegionListComponent implements OnInit {
  regions: Region[] = [];

  constructor(private regionService: RegionService) { }

  ngOnInit(): void {
    this.regionService.getAllRegions().subscribe((data: Region[]) => {
      this.regions = data;
      console.log(this.regions); // Adicione esta linha para verificar os dados recebidos
    });
  }

  deleteRegion(ddd: string): void {
    if (confirm('Tem certeza de que deseja deletar esta região?')) {
      this.regionService.deleteRegion(ddd).subscribe(() => {
        this.regions = this.regions.filter(region => region.ddd !== ddd);
      });
    }
  }
}
