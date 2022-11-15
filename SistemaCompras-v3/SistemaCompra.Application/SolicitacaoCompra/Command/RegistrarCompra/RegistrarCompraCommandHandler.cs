using MediatR;
using SistemaCompra.Application.Produto.Command.RegistrarProduto;
using SistemaCompra.Infra.Data.Produto;
using SistemaCompra.Infra.Data.UoW;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SolicitacaoCompraAgg = SistemaCompra.Domain.SolicitacaoCompraAggregate;
using ProdutoAgg = SistemaCompra.Domain.ProdutoAggregate;
using SistemaCompra.Domain.ProdutoAggregate;
using SistemaCompra.Domain.SolicitacaoCompraAggregate;

namespace SistemaCompra.Application.SolicitacaoCompra.Command.RegistrarCompra
{
    public class RegistrarCompraCommandHandler : CommandHandler, IRequestHandler<RegistrarCompraCommand, bool>
    {
        private readonly SolicitacaoCompraAgg.ISolicitacaoCompraRepository solicitacaoCompraAggRepository;
        private readonly IProdutoRepository _produtoRepository;

        public RegistrarCompraCommandHandler(SolicitacaoCompraAgg.ISolicitacaoCompraRepository solicitacaoCompraAggRepository, ProdutoAgg.IProdutoRepository produtoRepository, IUnitOfWork uow, IMediator mediator) : base(uow, mediator)
        {
            this.solicitacaoCompraAggRepository = solicitacaoCompraAggRepository;
            _produtoRepository = produtoRepository;
        }

        public Task<bool> Handle(RegistrarCompraCommand request, CancellationToken cancellationToken)
        {
            var solicitacaoCompra = new SolicitacaoCompraAgg.SolicitacaoCompra(request.UsuarioSolicitante, request.NomeFornecedor);
            
            foreach (var i in request.Itens)
            {
                var produto = _produtoRepository.Obter(i.Produto.Id);
                solicitacaoCompra.Itens.Add(new Item
                {
                    Qtde = i.Qtde,
                    Produto = produto
                });
            }
                
            solicitacaoCompraAggRepository.RegistrarCompra(solicitacaoCompra);

            Commit();
            PublishEvents(solicitacaoCompra.Events);

            return Task.FromResult(true);
        }
    }
}
