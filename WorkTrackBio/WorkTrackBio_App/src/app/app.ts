import { Component, signal } from '@angular/core';
import { Main } from './modules/shared/layouts/main/main';

@Component({
  selector: 'app-root',
  imports: [Main],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('WorkTrackBio_App');
}
