// Exceção para quando algo já existe (Erro 409)
public class ConflictException : Exception 
{ 
    public ConflictException(string message) : base(message) { } 
}

// Exceção para erros de validação (Erro 400)
public class ValidationException : Exception 
{ 
    public ValidationException(string message) : base(message) { } 
}