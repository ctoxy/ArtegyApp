using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController:BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            
        }
        
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() 
        {
            var users = await _userRepository.GetMembersAsync(); 
            return Ok(users); 
                     
        }
        
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username) 
        {
           return await _userRepository.GetMemberAsync(username);
             
        }


        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
        {
            //récupération du user par son token en cible
           var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           // recupération du user en db issu du token au dessus
           var user = await _userRepository.GetUserByUsernameAsync(username);

           if (user == null)
           {
             return NotFound();
           }
            //comparaison via automapper des deux objets
           _mapper.Map(memberUpdateDto, user);
            // code 204 rien a retourner
           if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("echec de mise a jour du user meme contenu déja"); 
        }
    }
}