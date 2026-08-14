import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../Services/user-service';
import { ItemService } from '../../Services/item-service';
import { Item } from '../../Models/item';
import { ItemDto } from '../../Dtos/item-dto';
import { UserDto } from '../../Dtos/user-dto';
import { User } from '../../Models/user';
import { OrderService } from '../../Services/order-service';

@Component({
  selector: 'app-home-component',
  imports: [],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css',
})
export class HomeComponent implements OnInit {

  items=signal<ItemDto[]>([])
  currentUser=signal<User |null >(null)
  constructor(private router:Router,private userService:UserService,
    private itemService:ItemService,private orderService:OrderService){}
  ngOnInit(): void {
    this.itemService.GetAllItems().subscribe({
      next:(res)=>{
        this.items.set(res)
      }
    })
    this.userService.GetCurrentUser().subscribe({
      next:(res)=>{
        this.currentUser.set(res)
      },
      error:(err:Error)=>{
        console.log("error")
      }
    })
  }

  ViewDetails(itemId:number){
    this.router.navigate(['items',itemId])
  }

  GetAllOrders(){
    this.orderService.GetAllOrders().subscribe({
      next:(res)=>{
        this.router.navigateByUrl("orders")
      },
      error:(err:Error)=>{
        console.log(err)
      }
    })
  }

}
