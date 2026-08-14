import { Component, signal } from '@angular/core';
import { RouterOutlet,RouterLink } from '@angular/router';
import { FormsModule ,FormControl } from '@angular/forms';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet,FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('app');
}
