﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using ApiProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiProject.Models;
using AutoMapper;

namespace ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        private IAmazonDynamoDB dynamoDBClient;
        private readonly IMapper _mapper;

        public HomeController(IAmazonDynamoDB dynamoDBClient, IMapper mapper)
        {
            this.dynamoDBClient = dynamoDBClient;
            this._mapper = mapper;
            new AWSServices(dynamoDBClient).CreateTable();
        }

        //GetAll
        [HttpGet]
        [HttpGet("/api/getAll.{format}"), FormatFilter]
        public ActionResult<Task<List<EmployeeDTO>>> GetAll()
        {
            
            var results = _mapper.Map<IEnumerable<EmployeeDTO>>(new AWSServices(dynamoDBClient).GetAll().Result);
            return Ok(results);
        }

        //GetById
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDTO>> GetById(string id)
        {
            var employee = await new AWSServices(dynamoDBClient).GetById(id);

            if (employee == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<EmployeeDTO>(employee);
            return Ok(result);
        }

        //Post
        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> CreateEmployee(EmployeeDTO dto)
        {
            var item = _mapper.Map<Employee>(dto);
            Employee e = await new AWSServices(dynamoDBClient).CreateEmployee(item);
            var eDTO = _mapper.Map<EmployeeDTO>(e);

            return CreatedAtAction(nameof(GetById), new { id = eDTO.Id }, eDTO);
        }

        //Put
        [HttpPut]
        public async Task<ActionResult<Employee>> UpdateItem(string _Id, Employee item)
        {
            var employee = await new AWSServices(dynamoDBClient).UpdateEmployee(_Id, item);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        // Delete
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            var employee = new AWSServices(dynamoDBClient).DeleteEmployee(id).Result;
        }
    }
}
