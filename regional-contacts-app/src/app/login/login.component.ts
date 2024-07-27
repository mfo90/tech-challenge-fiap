import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';  // Adicionada variável para armazenar mensagens de erro

  constructor(private authService: AuthService) { }

  onSubmit() {
    this.authService.login(this.username, this.password).subscribe(
      () => {
        // Sucesso - o redirecionamento é tratado no AuthService
        this.errorMessage = '';
      },
      error => {
        // Erro - exibir mensagem de erro
        this.errorMessage = 'Não foi possível realizar o login. Verifique suas credenciais e tente novamente.';
        console.error('Login failed', error);
      }
    );
  }
}
