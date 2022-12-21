import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
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
  // for template form
  //model: any = {};
  // creation du reactive Form
  registerForm: FormGroup = new FormGroup({});

  maxDate:Date = new Date();

  validationErrors: string[] | undefined;

  constructor(private accountService: AccountService, 
    private toastr:ToastrService, 
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    //pour age - 18ans
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }

  initializeForm(){
    this.registerForm = this.fb.group({
      username: [
        'hello',
        Validators.required
      ],
      gender: [
        'male',        
      ],
      knownAs: [
        '',
        Validators.required
      ],
      dateOfBirth: [
        '',
        Validators.required
      ],
      city: [
        '',
        Validators.required
      ],
      country: [
        '',
        Validators.required
      ],
      password: [
          '', [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(10)
        ]
      ],
      confirmPassword: [
          '', [
          Validators.required,
          this.matchValues('password')
          ]
      ]
    });
    //permet de valider que le champs controle ici password correspond au second via observable
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValues(matchTo: string):ValidatorFn{
    return (control:AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null: {notMatching: true}
    }

  }

  register(){
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {...this.registerForm.value, dateOfBirth: dob};
    //console.log(values);
    
    // reactive Form
    console.log(this.registerForm?.value);
    this.accountService.register(values).subscribe({
      next: () => {        
        this.router.navigateByUrl('/members');        
      },
      error: error => {
        this.validationErrors= error;
        
      } 
    })
    // template form
    //console.log(this.model);
    /*this.accountService.register(this.model).subscribe({
      next: () => {        
        this.cancel();        
      },
      error: error => this.toastr.error(error.error)
      
    })*/
    
  }

  cancel(){
    this.cancelRegister.emit(false);    
  }

  getDateOnly(dob:string | undefined){
    if (!dob) {
      return
    }
    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset()))
        .toISOString().slice(0,10);

  }
}
