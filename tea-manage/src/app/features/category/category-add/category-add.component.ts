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
import {
  Category,
  CategoryAddChildren,
  CategoryAddParent,
} from '../../../shared/models/category';
import { ToastrService } from 'ngx-toastr';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { CommonModule } from '@angular/common';
import { UtilitiesService } from '../../../core/services/utilities.service';

@Component({
  selector: 'app-category-add',
  standalone: true,
  imports: [
    NzButtonModule,
    NzModalModule,
    NzFormModule,
    ReactiveFormsModule,
    NzInputModule,
    CommonModule,
  ],
  templateUrl: './category-add.component.html',
  styleUrl: './category-add.component.css',
})
export class CategoryAddComponent implements OnInit {
  parentId: number = 0;
  parentName: string = '';
  @Output() categoryAdded = new EventEmitter<Category>();
  private categoryService = inject(CategoryService);
  private toastrService = inject(ToastrService);
  private utilService = inject(UtilitiesService);
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
      parentName: [''],
    });

    this.frm.get('name')?.valueChanges.subscribe((name) => {
      const slug = this.utilService.generateSlug(name); // Tạo slug từ name
      this.frm.get('slug')?.setValue(slug, { emitEvent: false }); // Cập nhật trường slug
    });
  }

  showModal(): void {
    if (this.parentId !== 0 || this.parentName.length > 0) {
      this.frm.patchValue({
        parentName: this.parentName,
      });
    }
    this.isVisible = true;
  }

  submitForm(): void {
    if (this.parentId !== 0 || this.parentName.length > 0) {
      const categoryAddChildren: CategoryAddChildren = {
        name: this.frm.value.name,
        slug: this.frm.value.slug,
        description: this.frm.value.description,
        parentId: this.parentId,
      };

      this.categoryService.addChildren(categoryAddChildren).subscribe({
        next: (res) => {
          this.categoryAdded.emit(res as Category);
          this.handleCancel();
          this.toastrService.success('Thêm danh mục thành công');
        },
        error: (er) => {
          console.log(er);
          this.validationErrors = er;
        },
      });
    } else {
      const categoryAddParent: CategoryAddParent = {
        name: this.frm.value.name,
        slug: this.frm.value.slug,
        description: this.frm.value.description,
      };
      this.categoryService.addParent(categoryAddParent).subscribe({
        next: (res) => {
          this.categoryAdded.emit(res as Category);
          this.handleCancel();
          this.toastrService.success('Thêm danh mục thành công');
        },
        error: (er) => {
          console.log(er);
          this.validationErrors = er;
        },
      });
    }
  }

  handleCancel(): void {
    this.frm.reset();
    this.isVisible = false;
    this.validationErrors = [];
    this.parentId = 0;
    this.parentName = '';
  }
}
