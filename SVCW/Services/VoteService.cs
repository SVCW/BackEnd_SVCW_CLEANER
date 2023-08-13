using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Votes;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class VoteService : IVote
    {
        private readonly SVCWContext _context;
        public VoteService(SVCWContext context)
        {
            _context = context;
        }
        public async Task<Vote> createVote(VoteDTO vote)
        {
            try
            {
                var v = await this._context.Vote.Where(x=>x.UserId.Equals(vote.UserId) && x.UserVoteId.Equals(vote.UserVoteId)).FirstOrDefaultAsync();
                if (v != null)
                {
                    v.IsLike= true;
                    this._context.Vote.Update(v);
                    await this._context.SaveChangesAsync();
                    var check = await this._context.User.Where(x => x.UserId.Equals(vote.UserVoteId)).FirstOrDefaultAsync();
                    if (check != null)
                    {
                        check.NumberLike += 1;
                        this._context.User.Update(check);
                        await this._context.SaveChangesAsync();
                    }
                    return v;
                }
                var vte = new Vote();
                vte.VoteId = "VOTE"+Guid.NewGuid().ToString().Substring(0,6);
                vte.UserVoteId = vote.UserVoteId;
                vte.UserId= vote.UserId;
                if(vote.IsLike == true && vote.IsDislike == false)
                {
                    vte.IsLike= true;
                    vte.IsDislike = false;
                }else if(vote.IsLike == false && vote.IsDislike == true) 
                {
                    vte.IsLike= false;
                    vte.IsDislike = true;
                }

                await this._context.Vote.AddAsync(vte);
                if(await this._context.SaveChangesAsync()>0) 
                {
                    if (vote.IsLike == true && vote.IsDislike == false)
                    {
                        var check = await this._context.User.Where(x=>x.UserId.Equals(vote.UserVoteId)).FirstOrDefaultAsync();
                        if (check != null)
                        {
                            check.NumberLike += 1;
                            this._context.User.Update(check);
                            await this._context.SaveChangesAsync();
                        }
                    }
                    else if (vote.IsLike == false && vote.IsDislike == true)
                    {
                        var check = await this._context.User.Where(x => x.UserId.Equals(vote.UserVoteId)).FirstOrDefaultAsync();
                        if (check != null)
                        {
                            check.NumberDislike += 1;
                            this._context.User.Update(check);
                            await this._context.SaveChangesAsync();
                        }
                    }
                    return vte;
                }
                return null;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Vote> deleteVote(VoteDTO vote)
        {
            try
            {
                var check = await this._context.Vote.Where(x => x.VoteId.Equals(vote.VoteId)).FirstOrDefaultAsync();
                if(check != null)
                {
                    check.IsLike= false;
                    check.IsDislike= false;
                    var check1 = await this._context.User.Where(x => x.UserId.Equals(vote.UserVoteId)).FirstOrDefaultAsync();
                    if (check1 != null)
                    {
                        check1.NumberDislike -= 1;
                        check1.NumberLike -= 1;
                        this._context.User.Update(check1);
                        await this._context.SaveChangesAsync();
                    }
                    this._context.Vote.Update(check);
                    await this._context.SaveChangesAsync();
                    return check;
                }
                return null;
            }catch(Exception ex) 
            { 
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Vote>> getAll()
        {
            try
            {
                var check = await this._context.Vote.ToListAsync();
                if(check.Count > 0)
                {
                    return check;
                }
                return null;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Vote> updateVote(VoteDTO vote)
        {
            try
            {
                var check= await this._context.Vote.Where(x=>x.VoteId.Equals(vote.VoteId)).FirstOrDefaultAsync();
                if(check != null)
                {
                    if (vote.IsLike == true && vote.IsDislike == false && check.IsLike==false)
                    {
                        check.IsLike = true;
                        check.IsDislike = false;
                        var check1 = await this._context.User.Where(x => x.UserId.Equals(vote.UserVoteId)).FirstOrDefaultAsync();
                        if (check1 != null)
                        {
                            check1.NumberDislike -= 1;
                            check1.NumberLike += 1;
                            this._context.User.Update(check1);
                            await this._context.SaveChangesAsync();
                        }
                    }
                    else if (vote.IsLike == false && vote.IsDislike == true && check.IsDislike == false)
                    {
                        check.IsLike = false;
                        check.IsDislike = true;
                        var check1 = await this._context.User.Where(x => x.UserId.Equals(vote.UserVoteId)).FirstOrDefaultAsync();
                        if (check1 != null)
                        {
                            check1.NumberDislike += 1;
                            check1.NumberLike -= 1;
                            this._context.User.Update(check1);
                            await this._context.SaveChangesAsync();
                        }
                    }
                }
                this._context.Vote.Update(check);
                await this._context.SaveChangesAsync();
                return check;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
