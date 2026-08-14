import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CategoryDto } from '../Dtos/category-dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {

  constructor(private httpClient:HttpClient){}

  CreateCategory(categoryDto:CategoryDto):Observable<CategoryDto>{
    return this.httpClient.post<CategoryDto>(`https://localhost:7273/api/categories`,categoryDto)
  }
  GetAllCategories():Observable<CategoryDto[]>{
  return this.httpClient.get<CategoryDto[]>(`https://localhost:7273/api/categories`)
  }
  GetCategory(categoryId:number):Observable<CategoryDto>{
    return this.httpClient.get<CategoryDto>(`https://localhost:7273/api/categories/${categoryId}`)
  }
  DeleteCategory(categoryId:number):Observable<void>{
    return this.httpClient.delete<void>(`https://localhost:7273/api/categories/${categoryId}`)
  }

  UpdateCategory(categoryId:number,categoryDto:CategoryDto):Observable<CategoryDto>{
    return this.httpClient.put<CategoryDto>(`hhttps://localhost:7273/api/categories/${categoryId}?newName=${categoryDto.name}&newDescription=${categoryDto.description}`,categoryDto)
  }
  
}
