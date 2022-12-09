import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // passage parent(home) to child(register) 
  //@Input() usersFromHomeComponent: any;
  // passage enfant(register) to parent(home)
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  
  constructor(private accountService: AccountService,private toastr:ToastrService) { }

  ngOnInit(): void {
  }

  register(){
    //console.log(this.model);
    this.accountService.register(this.model).subscribe({
      next: () => {        
        this.cancel();        
      },
      error: error => this.toastr.error(error.error)
      
    })
    
  }

  cancel(){
    this.cancelRegister.emit(false);
    
  }
}
