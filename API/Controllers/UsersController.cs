using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
        private readonly IPhotoService _photoService;
        
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
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

        //MISE A JOUR PROFIL USER
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
        {
            //récupération du user par son token en cible via extension ClaimsPrincipalExtensions
           var username = User.GetUsername();
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
        // UPLOAD PHOTO FOR USER
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file) 
        {
             //récupération du user par son token en cible via extension ClaimsPrincipalExtensions
       
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null)
            {
                return NotFound();
            }

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);
            
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            
            // si une seul photo = main photo
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser),
                 new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            };

            return BadRequest("Problem ajout de la photo");

        }

        // set main photo for user
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId) 
        {
            //récupération du user par son token en cible via extension ClaimsPrincipalExtensions       
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null)
            {
                return NotFound();
            }
            //recupération de la photoId
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null)
            {
                return NotFound();
            }
            if (photo.IsMain)
            {
                return BadRequest("Cette photo est déja la principale");
            }
            // si la photo actuel est déja la principale
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            // on a déja une photo qui est déja principale
            if (currentMain != null)
            {
                currentMain.IsMain = false;
            }
            // met la nouvelle photo en principale
            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem pour mettre en photo principale");

        }

        //Delete a user Photo
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            //récupération du user par son token en cible via extension ClaimsPrincipalExtensions       
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo == null)  return NotFound();
            if (photo.IsMain)
            {
                return BadRequest("Cette photo est la principal ne peut pas etre effacer");
            }
            if (photo.PublicId !=null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem pour supprimer la photo");
        }
    }
}