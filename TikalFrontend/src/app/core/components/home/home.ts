import { Component, ChangeDetectionStrategy } from '@angular/core';
import { NgOptimizedImage } from '@angular/common';
import { Button } from '../button/button';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-home',
  imports: [NgOptimizedImage, Button],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {}
