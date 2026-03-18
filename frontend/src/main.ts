import { bootstrapApplication } from '@angular/platform-browser';
import { Component } from '@angular/core';
import { provideHttpClient, HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';

@Component({
selector: 'app-root',
standalone: true,
template: `
<h1>{{ message }}</h1>
`
})
class AppComponent {
message = 'Loading...';
http = inject(HttpClient);

ngOnInit() {
    this.http.get('http://localhost:5000', { responseType: 'text' })
      .subscribe(res => this.message = res);
  }
}

bootstrapApplication(AppComponent, {
  providers: [provideHttpClient()]
});
