import { Component } from '@angular/core';
import { SkeletonModule } from 'primeng/skeleton';

@Component({
  selector: 'tikal-skeleton',
  imports: [SkeletonModule],
  templateUrl: './skeleton.html',
  styleUrl: './skeleton.scss',
})
export class SkeletonComponent {}
