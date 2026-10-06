# DVDClock Screensaver

Um protetor de tela inspirado no clássico logo quicando do DVD, mas com um relógio digital minimalista. Escrito em C# e WinForms (.NET 10).

## Compilação e Instalação

### Usando o Script de Build
1. Execute o arquivo `build.bat` localizado na raiz do projeto.
2. O script irá compilar o projeto em modo Release e gerar automaticamente o executável renomeado para `DVDClock.scr` na pasta `Release`.
3. Clique com o botão direito no arquivo `DVDClock.scr` e selecione **Instalar** (ou **Testar**, ou **Configurar**).

### Usando o Visual Studio 2022+ ou CLI
Você pode compilar o projeto também com:
```bash
dotnet build -c Release
```
Em seguida, basta ir à pasta `bin\Release\net10.0-windows\`, copiar o executável `DVDClock.exe` e colá-lo renomeado como `DVDClock.scr`.

### Configurações
Na tela de configurações do protetor de tela no Windows, você pode definir a cor e a velocidade do relógio.

### Funcionamento do .SCR
No Windows, os arquivos `.scr` (Screen Saver) são apenas executáveis normais (`.exe`) renomeados. O Windows gerencia o protetor de tela executando este arquivo com alguns parâmetros especiais de linha de comando:
- `/c`: Abre a tela de configuração do protetor de tela.
- `/s`: Inicia o protetor de tela em modo tela cheia.
- `/p <HWND>`: Modo de pré-visualização, onde o protetor desenha sua interface dentro da pequena janela de configurações no painel de controle do Windows.

A aplicação implementa os tratamentos destes parâmetros no arquivo `Program.cs`. No modo `/p`, usamos a API do Windows (P/Invoke) para integrar a janela (HWND filho) à janela do Windows (HWND pai).

### Desinstalação
1. Para desinstalar, basta excluir o arquivo `DVDClock.scr` de onde você o copiou. Se não quiser configurá-lo permanentemente como o seu protetor de tela, não o mova para a pasta do sistema. 
2. As configurações são salvas em `%AppData%\DVDClock\settings.json`. Exclua a pasta `DVDClock` localizada em `%AppData%` (App Data -> Roaming) para apagar suas preferências.
