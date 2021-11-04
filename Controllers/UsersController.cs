using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Validation;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class UsersController : ControllerBase
    {
        private readonly WebApiContext _context;

        public UsersController(WebApiContext context)
        {
            _context = context;
        }


        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            var Users = new List<UserModel>();

            foreach (var _user in await _context.Users.Include(x => x.Profile).ToListAsync())
                Users.Add( item : new UserModel()
                {
                    Id = _user.Id,
                    Email = _user.Email,
                    FirstName = _user.Profile.FirstName,
                    LastName = _user.Profile.LastName
                });

            if (Users == null || Users.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Users;
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(int id)
        {
            var _user = await _context.Users.FindAsync(id);

            var _profile = await _context.Profiles.FindAsync(id);

            if (_user == null)
            {
                return NotFound();
            }
            else
            {
                var User = new UserModel()
                {
                    Id = _user.Id,
                    Email = _user.Email,
                    FirstName = _profile.FirstName,
                    LastName = _profile.LastName
                };
                return User;
            }

        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserCreateModel model)
        {
            if (id.GetType() != typeof(int))
            {
                return BadRequest();
            }
            else
            {
                var userExists = await _context.Users.Where(x => x.Email.ToLower() == model.Email.ToLower()).FirstOrDefaultAsync();

                if (userExists == null)
                {
                    if (Validations.IsValidEmail(model.Email))
                    {
                        if (model.FirstName.Length > 2 && model.LastName.Length > 2)
                        {
                            var UpdatedUser = new UserEntity()
                            {
                                Id = id,
                                Email = model.Email,
                            };

                            var UpdatedUserHash = new UserHashEntity()
                            {
                                UserId = id,
                                UserHash = model.UserHash
                            };

                            var UpdatedProfile = new ProfileEntity()
                            {
                                UserId = id,
                                FirstName = model.FirstName,
                                LastName = model.LastName
                            };

                            _context.Entry(UpdatedUser).State = EntityState.Modified;
                            _context.Entry(UpdatedUserHash).State = EntityState.Modified;
                            _context.Entry(UpdatedProfile).State = EntityState.Modified;

                            try
                            {
                                await _context.SaveChangesAsync();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                if (!UserEntityExists(id))
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    throw;
                                }
                            }

                            return NoContent();
                        }
                        else
                        {
                            return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"Firstname and Lastname must be atlease 2 characters long." }));
                        }
 
                    }
                    else
                    {
                        return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"${model.Email} is not a valid email address." }));
                    }
                }
                else
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"A user with that email already exists." }));
                }
            }
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserModel>> PostUser(UserCreateModel model)
        {
            if(!string.IsNullOrEmpty(model.Email) &&
                !string.IsNullOrEmpty(model.UserHash) &&
                !string.IsNullOrEmpty(model.FirstName) &&
                !string.IsNullOrEmpty(model.LastName))
            {

                var userExists = await _context.Users.Where(x => x.Email.ToLower() == model.Email.ToLower()).FirstOrDefaultAsync();

                if(userExists == null)
                {
                    if (Validations.IsValidEmail(model.Email))
                    {
                        if(Validations.IsValidPassword(model.UserHash))
                        {
                            if(model.FirstName.Length > 2 && model.LastName.Length > 2)
                            {
                                var user = new UserEntity
                                {
                                    Email = model.Email
                                };

                                _context.Users.Add(user);
                                await _context.SaveChangesAsync();
                        
                                var userHash = new UserHashEntity
                                {
                                    UserId = user.Id,
                                    UserHash = model.UserHash
                                };
                              
                                _context.UserHashes.Add(userHash);
                                await _context.SaveChangesAsync();

                                var userProfile = new ProfileEntity
                                {
                                    UserId = user.Id,
                                    FirstName = model.FirstName,
                                    LastName = model.LastName
                                };

                                _context.Profiles.Add(userProfile);
                                await _context.SaveChangesAsync();

                                return CreatedAtAction("GetUser", new { id = user.Id }, new UserModel
                                {
                                    Id = user.Id,
                                    Email = model.Email,
                                    FirstName = model.FirstName,
                                    LastName = model.LastName
                                });
                            }
                            else
                            {
                                return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"Firstname and Lastname must be atlease 2 characters long." }));
                            }
                        }
                        else
                        {
                            return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)"}));
                        }

                    }
                    else
                    {
                        return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"${model.Email} is not a valid email address." }));
                    }
                }
                else
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"A user with that email already exists." }));
                }
            }
            else
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"All fields must contains values." }));
            }           
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userEntity = await _context.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserEntityExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
