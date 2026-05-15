using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.Services;
using StoreWebApi.zAppContexts;
using System;
using System.Net.Http;
using System.Net;

namespace StoreTests
{
    public class ItemServiceTests
    {
        private HttpClient _httpClient;
        public ItemServiceTests()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5129");
        }

        [Fact]
        public async Task getItemWhenExist_shouldReturnItem()
        {
            //Arrange
            var ItemName="math";

            // Act
            var response = await _httpClient.GetAsync($"/api/items/{ItemName}");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(content));

        }
        [Fact]
        public async Task GetItem_ShouldThrowException_WhenItemDoesNotExist()
        {
            var ItemName = "blablabla";

            // Act
            var response = await _httpClient.GetAsync($"/api/items/{ItemName}");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(content));
        }
    }
}
