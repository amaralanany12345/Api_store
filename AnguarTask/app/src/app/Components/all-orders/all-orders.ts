import { Component, OnInit, signal } from '@angular/core';
import { Order } from '../../Models/order';
import { OrderService } from '../../Services/order-service';
import { OrderDto } from '../../Dtos/order-dto';
import { OrderItemDto } from '../../Dtos/order-item-dto';
import { Router } from '@angular/router';

@Component({
  selector: 'app-all-orders',
  imports: [],
  templateUrl: './all-orders.html',
  styleUrl: './all-orders.css',
})
export class AllOrders implements OnInit {

  constructor(private orderService:OrderService,private router:Router){}
  orders=signal<OrderDto[]>([])
  orderItems=signal<OrderItemDto[]>([])
  ngOnInit(): void {
    this.orderService.GetAllOrders().subscribe({
      next:(res)=>{
        this.orders.set(res)
      } 
    })
  }

  ViewDetails(orderId:number){
    // this.router.navigate(['items',itemId])
    this.router.navigate(['orders',orderId])
  }

}
