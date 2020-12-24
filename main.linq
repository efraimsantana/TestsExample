void Main()
{
	StartTests();
}

public void StartTests()
{
	var me = Assembly.GetExecutingAssembly();
	var result = Xunit.Runner.LinqPad.XunitRunner.Run(me);
	result.Dump("Xunit runner result");
}

public class RiscoAppServiceTests
{
	private AutoMocker _mocker;
	private readonly RiscoAppService _riscoAppService;
	private readonly Mock<IRiscoRepository> _riscoRepository;
	
	public RiscoAppServiceTests()
	{
		_mocker = new AutoMocker();
		_riscoRepository = new Mock<IRiscoRepository>();
		_riscoRepository.Setup(a => a.Adicionar(It.IsAny<Risco>())).Returns(true);
		
		_riscoAppService = new RiscoAppService(_riscoRepository.Object);
	}

	[Fact]
	public void DeveRetornaFalseValidacaoRiscoFalhar()
	{
		var riscoInvalido =  RiscoBuilder.Novo().ComNome(string.Empty).Build();

		var resultado = _riscoAppService.Registrar(riscoInvalido);

		Assert.False(resultado);
	}
	
	[Fact]
    public void DeveChamarRepositoryComParametroCorreto()
	{	
		var risco = RiscoBuilder.Novo().Build();
				
		_riscoAppService.Registrar(risco);

		_riscoRepository.Verify(mock => mock.Adicionar(risco), Moq.Times.Once());
    }

	[Fact]
	public void DeveRetornaTrueAoExecutarComSucesso()
	{
		var risco =  RiscoBuilder.Novo().Build();

		var resultado = _riscoAppService.Registrar(risco);

		Assert.True(resultado);
	} 
}

public class Risco
{
	public string Nome { get; private set; }
	
	public Risco(string nome)
	{
		Nome = nome;
	}
}

public class RiscoAppService
{
	private readonly IRiscoRepository _riscoRepository;
	
	public RiscoAppService(IRiscoRepository riscoRepository)
	{
		_riscoRepository = riscoRepository;
	}
	
	public bool Registrar(Risco risco)
	{
		if (risco == null || string.IsNullOrWhiteSpace(risco.Nome))
		{
			return false;	
		}
		
		var resultado = _riscoRepository.Adicionar(risco);
		return resultado;
	}
}

public interface IRiscoRepository
{
	bool Adicionar(Risco risco);	
}

public class RiscoBuilder
{
	protected String Nome;
	
	public static RiscoBuilder Novo()
	{
		return new RiscoBuilder
		{
			Nome = "Código Não Testado"	
		};
	}
	
	public Risco Build()
	{
		return new Risco(this.Nome);
	}
	
	public RiscoBuilder ComNome(string nome)
	{
		Nome = nome;
		return this;
	}
}
