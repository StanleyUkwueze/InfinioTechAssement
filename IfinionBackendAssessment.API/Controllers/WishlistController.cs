﻿using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.WishListServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IfinionBackendAssessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController(IWishListService wishListService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> AddItemToWishList([FromForm] AddWishlistDto addWishlistDto)
        {
            var response = await wishListService.AddWishListItem(addWishlistDto);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ViewWishListItems()
        {
            var response = await wishListService.GetAllWishlistItems();
            return Ok(response);
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> RemoveItemFromWishList(int itemId)
        {
            var response = await wishListService.RemoveItem(itemId);
            return Ok(response);
        }
    }
}
