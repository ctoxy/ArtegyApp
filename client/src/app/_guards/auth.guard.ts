import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { map, Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})

// un guard souscrit automatiquement a une observable
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService, private toastr:ToastrService){}
  canActivate():Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if(user) return true;
        else {
          this.toastr.error('Accés Interdit par le Guard');
          return false
        }
      })
    )
  }  
}
