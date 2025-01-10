import { Component } from '@angular/core';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { UserManagementComponent } from '../user-management/user-management.component';
import { HasRoleDirective } from '../../_directives/has-role.directive';
import { PhotoManagementComponent } from "../photo-management/photo-management.component";

@Component({
    standalone:true,
    selector: 'app-admin-panel',
    imports: [TabsModule, UserManagementComponent, HasRoleDirective, PhotoManagementComponent],
    templateUrl: './admin-panel.component.html',
    styleUrl: './admin-panel.component.css'
})
export class AdminPanelComponent {

}
