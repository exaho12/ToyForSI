using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToyForSI.Data;
using ToyForSI.Models;
using System;

namespace ToyForSI.Controllers
{
    public class DeviceController : Controller
    {
        private readonly ToyForSIContext _context;

        public DeviceController(ToyForSIContext context)
        {
            _context = context;
        }

        // GET: Device
        public async Task<IActionResult> Index()
        {
            var toyForSIContext = _context.Device.Include(d => d.devModel);
            return View(await toyForSIContext.ToListAsync());
        }

        // GET: Device/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Device
                .Include(d => d.devModel)
                .SingleOrDefaultAsync(m => m.deviceId == id);
            if (device == null)
            {
                return NotFound();
            }
            ViewData["SiSN"] = device.siSN;
            return View(device);
        }

        // GET: Device/Create
        public IActionResult Create()
        {
            ViewData["devModelId"] = new SelectList(_context.DevModel, "devModelId", "devModelName");
            return View();
        }

        // POST: Device/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("deviceId,contractNo,devModelId,createTime,inWareHouse,devCount")] MultiDev multidev)
        {
            if (ModelState.IsValid)
            {
                for (int idx = 0; idx != multidev.devCount; idx++)
                {
                    DevModel devModel = _context.DevModel.FirstOrDefault(d => d.devModelId == multidev.devModelId);
                    Device device = 
                        new Device { contractNo = multidev.contractNo,
                            devModelId = multidev.devModelId,
                            createTime = multidev.createTime,
                            inWareHouse = multidev.inWareHouse,
                        };
                    _context.Add(device);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["devModelId"] = new SelectList(_context.DevModel, "devModelId", "devModelName", multidev.devModelId);
            return View(multidev);
        }

        // GET: Device/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Device.SingleOrDefaultAsync(m => m.deviceId == id);
            if (device == null)
            {
                return NotFound();
            }
            ViewData["devModelId"] = new SelectList(_context.DevModel, "devModelId", "devModelName", device.devModelId);
            return View(device);
        }

        // POST: Device/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("deviceId,contractNo,devModelId,createTime,inWareHouse")] Device device)
        {
            if (id != device.deviceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.deviceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["devModelId"] = new SelectList(_context.DevModel, "devModelId", "devModelName", device.devModelId);
            return View(device);
        }

        // GET: Device/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Device
                .Include(d => d.devModel)
                .SingleOrDefaultAsync(m => m.deviceId == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Device/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.Device.SingleOrDefaultAsync(m => m.deviceId == id);
            _context.Device.Remove(device);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceExists(int id)
        {
            return _context.Device.Any(e => e.deviceId == id);
        }
    }
}