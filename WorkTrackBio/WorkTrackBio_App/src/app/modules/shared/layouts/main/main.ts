import { Component } from '@angular/core';
import { sharedImports } from '../../../shared/shared';

@Component({
  selector: 'app-main',
    imports: [
    ...sharedImports
  ],
  templateUrl: './main.html',
  styleUrl: './main.scss'
})
export class Main {

}