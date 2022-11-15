using SistemaCompra.Domain.Core;
using SistemaCompra.Domain.Core.Model;
using SistemaCompra.Domain.ProdutoAggregate;
using SistemaCompra.Domain.ProdutoAggregate.Events;
using SistemaCompra.Domain.SolicitacaoCompraAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaCompra.Domain.SolicitacaoCompraAggregate
{
    public class SolicitacaoCompra : Entity
    {
        public UsuarioSolicitante UsuarioSolicitante { get; private set; }
        public NomeFornecedor NomeFornecedor { get; private set; }
        public IList<Item> Itens { get; private set; }
        public DateTime Data { get; private set; }
        public Money TotalGeral { get; private set; }
        public Situacao Situacao { get; private set; }
        public int CondicaoPagamento { get; private set; }

        private int _CondicaoPagamentoPadrao = 30;
        private Money _CondicaoTotalParaPagamento = new Money(50000);

        private SolicitacaoCompra() { }

        public SolicitacaoCompra(string usuarioSolicitante, string nomeFornecedor)
        {
            Id = Guid.NewGuid();
            UsuarioSolicitante = new UsuarioSolicitante(usuarioSolicitante);
            NomeFornecedor = new NomeFornecedor(nomeFornecedor);
            Data = DateTime.Now;
            Situacao = Situacao.Solicitado;
        }

        public void AdicionarItem(Produto produto, int qtde)
        {
            Itens.Add(new Item(produto, qtde));
        }

        public void RegistrarCompra(IEnumerable<Item> itens)
        {
            if (ValidarCompra(itens))
            {
                TotalGeral = new Money(itens.Sum(i => i.Produto.Preco.Value * i.Qtde));
                CondicaoPagamento = TotalGeral.Value >= _CondicaoTotalParaPagamento.Value ? _CondicaoPagamentoPadrao : 0;

                AddEvent(new CompraRegistradaEvent(Id, itens, TotalGeral.Value));
            } else 
            {
                throw new BusinessRuleException("A solicitação de compra deve possuir itens!");
            }
        }

        private bool ValidarCompra(IEnumerable<Item> itens)
        {
            return itens.Count() > 0 ? true : false;
        }
    }
}
