using CustomJwt.Constants;
using CustomJwt.Entities;

namespace CustomJwt.Repositories;

public static class DataInitializer
{
    public static void Run(DataContext db)
    {
        if (db.User.Any()) return;
        
        // create role
        var adminRole = new Role
        {
            Name = "Administrator"
        };
        db.Role.Add(adminRole);
        
        var operatorRole = new Role
        {
            Name = "Operator"
        };
        db.Role.Add(operatorRole);

        db.SaveChanges();
        
        
        // create privileges
        db.RolePrivilege.Add(new RolePrivilege
        {
            RoleId = adminRole.Id,
            Privilege = PrivilegeConst.ReadUser
        });
        
        db.RolePrivilege.Add(new RolePrivilege
        {
            RoleId = adminRole.Id,
            Privilege = PrivilegeConst.CreateUser
        });
        
        db.RolePrivilege.Add(new RolePrivilege
        {
            RoleId = operatorRole.Id,
            Privilege = PrivilegeConst.ReadUser
        });

        db.SaveChanges();
        
        
        // create user
        var joanna = new User
        {
            Username = "joanna",
            Password = "joanna",
            RoleId = adminRole.Id
        };
        db.User.Add(joanna);
        
        var natasha = new User
        {
            Username = "natasha",
            Password = "natasha",
            RoleId = operatorRole.Id
        };
        db.User.Add(natasha);
        
        db.SaveChanges();
    }
}