import { Component, inject, signal } from '@angular/core';
import { LoginDto } from '../../Dtos/login-dto';
import { FormBuilder,ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../Services/user-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signin-component',
  imports: [ReactiveFormsModule],
  templateUrl: './signin-component.html',
  styleUrl: './signin-component.css',
})
export class SigninComponent {
  // loginDto:LoginDto={} as LoginDto
  error=signal<string|null>(null)
  constructor(private userService:UserService,private router:Router){

  }
  private formBuilder=inject(FormBuilder)

  signInForm=this.formBuilder.group({
    email:'' ,
    password:'',
  })

  signIn(){
    const loginDto:LoginDto={} as LoginDto
    loginDto.email=this.signInForm.getRawValue().email
    loginDto.password=this.signInForm.getRawValue().password
    this.userService.SignIn(loginDto).subscribe({
      next:(res)=>{
        this.router.navigateByUrl('home')
        console.log(res)
      },
      error:(err:Error)=>{
        this.error.set(err.message)
      }
    })
  }

}
