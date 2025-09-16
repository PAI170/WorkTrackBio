import { Component } from '@angular/core';
import { sharedImports } from '../../shared';
import {House, Search } from 'lucide-angular';
@Component({
  selector: 'app-main',
  standalone: true,
  imports: [...sharedImports],
  templateUrl: './main.html',
  styleUrl: './main.scss'
})
export class Main {
  searchTerm: string='';
  
  protected readonly House = House;
  protected readonly Search = Search;
}