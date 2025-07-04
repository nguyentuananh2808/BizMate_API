﻿using BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipt;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.InventoryReceipt.GetInventoryReceipt
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.InventoryReceipt)]
    [ApiController]
    [Authorize]
    public class InventoryReceiptController : ControllerBase
    {
        private readonly GetInventoryReceiptPresenter _presenter;
        private readonly IMediator _mediator;

        public InventoryReceiptController(GetInventoryReceiptPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetInventoryReceipt(Guid id)
        {
            var request = new GetInventoryReceiptRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
