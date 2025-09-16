import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';

export const sharedImports = [
  CommonModule,
  FormsModule,
  ReactiveFormsModule,
  LucideAngularModule
] as const;

export const sharedExports = [
  CommonModule,
  FormsModule,
  ReactiveFormsModule,
  LucideAngularModule
] as const;

export type SharedImports = (typeof sharedImports)[number];
export type SharedExports = (typeof sharedExports)[number];