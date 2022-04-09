using CustomJwt.Attributes;
using CustomJwt.Constants;
using CustomJwt.Entities;
using CustomJwt.Exceptions;
using CustomJwt.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomJwt.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly DataContext _db;

    public UserController(DataContext db)
    {
        _db = db;
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult Get(int id)
    {
        var response = _db.User.Find(id);

        if (response == null) throw new NotFoundException("User not found.");
        
        return Ok(response);
    }
    
    [Authorize(PrivilegeConst.ReadUser)]
    [HttpGet]
    public ActionResult GetAll()
    {
        var response = _db.User.Include(x => x.Role).ToList();
        return Ok(response);
    }

    [Authorize(PrivilegeConst.CreateUser)]
    [HttpPost]
    public ActionResult Create([FromBody] User user)
    {
        _db.User.Add(user);
        _db.SaveChanges();
        return Ok(user);
    }
}