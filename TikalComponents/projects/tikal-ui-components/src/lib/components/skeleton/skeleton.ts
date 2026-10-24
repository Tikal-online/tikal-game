import { Component } from '@angular/core';
import { SkeletonModule } from '@openng/optimus-ui/skeleton';

@Component({
  selector: 'tikal-skeleton',
  imports: [SkeletonModule],
  templateUrl: './skeleton.html',
  styleUrl: './skeleton.scss',
})
export class SkeletonComponent {}
