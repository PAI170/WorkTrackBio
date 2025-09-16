import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LucideAngularModule, House, Search } from 'lucide-angular';

export const sharedImports = [
  CommonModule,
  FormsModule,
  ReactiveFormsModule,
  LucideAngularModule.pick({ House, Search })
] as const;

export type SharedImports = (typeof sharedImports)[number];


