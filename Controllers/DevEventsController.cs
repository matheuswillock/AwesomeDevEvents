using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDbContext _context;

        public DevEventsController(DevEventsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter todos os eventos
        /// </summary>
        /// <returns>Coleção de eventos</returns>
        /// <response code="200">Sucesso</response> 
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult GetAll()
        {
            var devEvents = _context.DevEvents.Where(d => (bool)!d.IsDeleted!).ToList();

            foreach (var devEvent in devEvents)
            {
                var id = devEvent.Id;

                var speakers = _context.DevEventSpeakers.Where(speaker => speaker.DevEventId == id).ToList();

                devEvent.Speakers = speakers;
            }

            return Ok(devEvents);
        }

        /// <summary>
        /// obter um evento pelo Id
        /// </summary>
        /// <param name="id">Identificador</param>
        /// <returns>Dados de um evento</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _context.DevEvents
                .Include(d => d.Speakers)
                .SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
                return NotFound();

            return Ok(devEvent);
        }

        /// <summary>
        /// Cadastrar um evento
        /// </summary>
        /// <remarks>
        /// { "title": "", "description": "", "startDate": "2023-07-10T16:16:21.643Z", "endDate": "2023-07-10T16:16:21.643Z" }
        /// </remarks>
        /// <param name="devEvent">Dados do evento</param>
        /// <returns>Objeto recém criado</returns>
        /// /// <response code="201">Sucesso na criação</response> 
        /// 
        [HttpPost]
        [ProducesResponseType(201)]
        public IActionResult Post(DevEvent devEvent)
        {
            _context.DevEvents.Add(devEvent);

            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        /// <summary>
        /// Atualizar um evento
        /// </summary>
        /// /// <remarks>
        /// { "title": "", "description": "", "startDate": "2023-07-10T16:20:34.614Z", "endDate": "2023-07-10T16:20:34.614Z" }
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="devEvent">Dados do evento</param>
        /// <returns>Sem conteúdo</returns>
        /// /// <response code="204">Sem conteúdo</response> 
        /// /// <response code="404">Não encontrado</response>
        /// 
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Put(Guid id, DevEvent devEvent)
        {

            var devEventOld = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEventOld == null)
                return NotFound();

            devEventOld.Update(devEvent.Title, devEvent.Description, devEvent.StartDate, devEvent.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deletar um evento
        /// </summary>
        /// <param name="id">Identificador</param>
        /// <returns>Sem conteúdo</returns>
        /// <response code="204">Sem conteúdo</response>
        /// <response code="404">Não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Delete();

            _context.SaveChanges();

            return NoContent();
        }


        /// <summary>
        /// Cadastrar palestrante
        /// </summary>
        /// <remarks>
        /// { "name": "", "talkTitle": "", "linkedInProfile": "" }
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="speaker"></param>
        /// <returns>Objeto recém criado</returns>
        /// /// <response code="201">Sucesso na criação</response> 
        /// 
        [HttpPost("{id}/speakers")]
        [ProducesResponseType(201)]
        public IActionResult PostSpeaker(Guid id, DevEventSpeaker speaker)
        {
            speaker.DevEventId = id;

            var devEvent = _context.DevEvents.Any(d => d.Id == id);

            if (!devEvent)
                return NotFound();

            _context.DevEventSpeakers.Add(speaker);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Obter todos os palestrantes
        /// </summary>
        /// <returns>Coleção de palestrantes</returns>
        /// <response code="200">Sucesso</response> 
        [HttpGet("/speakers")]
        [ProducesResponseType(200)]
        public IActionResult GetAllSpeakers()
        {
            var speakers = _context.DevEventSpeakers.ToList();

            return Ok(speakers);
        }
    }
}
