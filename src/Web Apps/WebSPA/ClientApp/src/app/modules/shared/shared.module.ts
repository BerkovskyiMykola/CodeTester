import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ReactiveFormsModule} from "@angular/forms";
import {NgbTooltip} from "@ng-bootstrap/ng-bootstrap";
import { ModalComponent } from './components/modal/modal.component';
import { AutoFocusDirective } from './directives/auto-focus.directive';



@NgModule({
  declarations: [
    ModalComponent,
    AutoFocusDirective
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgbTooltip
  ],
  exports: [
    ModalComponent,
    AutoFocusDirective
  ]
})
export class SharedModule {
  static forRoot(): ModuleWithProviders<SharedModule> {
      return {
          ngModule: SharedModule,
          providers: []
      };
  }
}
