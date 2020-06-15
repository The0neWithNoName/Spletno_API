﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpletnoProject.BlockchainClasses
{
    public class Blockchain
    {

        public List<Block> chain = new List<Block>();

        public Blockchain()
        {

            this.chain.Add(new Block(0, "Init\n", ""));
        }


        public void addBlock(Block newBlock, int diff)
        {
            newBlock.prevHash = this.getLastBlock().hash;

            newBlock.mineBlock(diff);
            this.chain.Add(newBlock);
        }

        public Block getLastBlock()
        {
            return this.chain[this.chain.Count - 1];
        }

        public bool validateChain()
        {
            for (int i = 0; i < this.chain.Count; i++)
            {
                Block currBlock = this.chain[i];
                Block prevBlock = this.chain[i - 1];

                if (currBlock.hash != currBlock.calculateHash() || currBlock.prevHash != prevBlock.hash)
                {
                    return false;
                }
            }
            return true;
        }

        public void updateChain(Blockchain newChain)
        {
            this.chain.Clear();
            this.chain = newChain.chain;
        }

        public string toString()
        {
            string outString = "";
            for (int i = 1; i < this.chain.Count; i++)
            {
                outString += this.chain[i].toString();
            }
            return outString;
        }

        public string[] toStringArray()
        {
            string[] outString = new string[this.chain.Count - 1];
            for (int i = 0; i < this.chain.Count - 1; i++)
            {
                outString[i] = this.chain[i + 1].blockToJson();
            }
            return outString;
        }


    }
}