import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RegionService } from '../../../services/region.service';
import { Region } from '../../../services/region.model';

@Component({
  selector: 'app-region-edit',
  templateUrl: './region-edit.component.html',
  styleUrls: ['./region-edit.component.css']
})
export class RegionEditComponent implements OnInit {
  region: Region = { ddd: '', name: '' };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private regionService: RegionService
  ) { }

  ngOnInit(): void {
    const ddd = this.route.snapshot.paramMap.get('ddd');
    if (ddd) {
      this.regionService.getRegionByDDD(ddd).subscribe((data: Region) => {
        this.region = data;
      });
    }
  }

  updateRegion(): void {
    this.region.ddd = this.region.ddd.toString()
    this.regionService.updateRegion(this.region).subscribe(() => {
      this.router.navigate(['/']);
    });
  }
}
