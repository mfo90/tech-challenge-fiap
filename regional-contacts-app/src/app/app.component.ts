import { Component, OnInit } from '@angular/core';
import { Region } from '../services/region.model';
import { RegionService } from '../services/region.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  regions: Region[] = [];
  title = 'App Contatos por Região';

  constructor(private regionService: RegionService, private authService: AuthService) {}

  ngOnInit() {
    this.regionService.getAllRegions().subscribe((data: Region[]) => {
      this.regions = data;
    });
  }

  logout() {
    this.authService.logout();
  }
}
