function digitButton (digit)
      return Button{ label = digit,
                     action = function ()
                                add_to_display(digit)
                              end
                   }
    end