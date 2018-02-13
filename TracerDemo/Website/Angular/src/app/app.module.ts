import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { Api } from './services/api';
import { AppService } from './services/appService';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ModalModule } from 'ngx-modialog';
import { BootstrapModalModule } from 'ngx-modialog/plugins/bootstrap';


import { AppComponent } from './components/app/app.component';
import { HomeComponent } from './components/home/home.component';
import { TodoComponent } from './components/todo/todo.component';
import { AccountComponent } from './components/account/account.component';
import { RegisterComponent } from './components/register/register.component';
import { SignInComponent } from './components/signin/signin.component';

@NgModule({
    declarations: [
        AppComponent,
        RegisterComponent,
        SignInComponent,
        TodoComponent,
        AccountComponent,
        HomeComponent
    ],
    imports: [
        BrowserModule,
        CommonModule,
        HttpModule,
        FormsModule,
        NgbModule.forRoot(),
        ModalModule.forRoot(),
        BootstrapModalModule,
        RouterModule.forRoot([
            { path: 'home', redirectTo: '', pathMatch: 'full' },
            { path: '', component: HomeComponent },
            { path: 'register', component: RegisterComponent },
            { path: 'signin', component: SignInComponent },
            { path: 'todo', component: TodoComponent },
            { path: 'account', component: AccountComponent },
            { path: '**', redirectTo: '' }
        ])
    ],
    providers: [
        Api,
        AppService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
