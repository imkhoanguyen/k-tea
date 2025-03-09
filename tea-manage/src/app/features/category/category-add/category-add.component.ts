import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { CategoryService } from '../../../core/services/category.service';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Category, CategoryAddParent } from '../../../shared/models/category';
import { ToastrService } from 'ngx-toastr';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';

@Component({
  selector: 'app-category-add',
  standalone: true,
  imports: [
    NzButtonModule,
    NzModalModule,
    NzFormModule,
    ReactiveFormsModule,
    NzInputModule,
  ],
  templateUrl: './category-add.component.html',
  styleUrl: './category-add.component.css',
})
export class CategoryAddComponent implements OnInit {
  @Output() categoryParentAdded = new EventEmitter<Category>();
  private categoryService = inject(CategoryService);
  private toastrService = inject(ToastrService);
  isVisible = false;
  private fb = inject(FormBuilder);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.frm = this.fb.group({
      name: ['', Validators.required],
      slug: ['', Validators.required],
      description: [''],
    });
  }

  showModal(): void {
    this.isVisible = true;
  }

  submitForm(): void {
    const categoryAddParent: CategoryAddParent = {
      name: this.frm.value.name,
      slug: this.frm.value.slug,
      description: this.frm.value.description,
    };
    this.categoryService.addParent(categoryAddParent).subscribe({
      next: (res) => {
        this.categoryParentAdded.emit(res as Category);
        this.handleCancel();
        this.toastrService.success('Thêm danh mục thành công');
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  handleCancel(): void {
    this.frm.reset();
    this.isVisible = false;
  }
}
