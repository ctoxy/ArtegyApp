import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  //currentUser$: Observable<User | null> = of(null);
  // public car utiliser dans le template
  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
    //this.currentUser$ = this.accountService.currentUser$;
  }

  

  login() {
    //console.log(this.model);
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
        
      },
      error: error => console.log(error)
    })
  }
  logout() {
    //console.log(this.model);
    this.accountService.logout();
   
    
  }

}